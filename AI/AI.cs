using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
using NumSharp;

namespace KeepLordWarriors.AI
{
    public static class Constants
    {
        public const int ACTION_SIZE = 5;  // 0%, 25%, 50%, 75%, 100%
        public const int NUM_KEEPS = 64;
        public const int MAP_SIZE = 256;
        public const int NUM_TILES = 1; // Traversable or not
        public const int NUM_PLAYERS = 20;
        public const int TOTAL_ACTION_SPACE = NUM_KEEPS * NUM_KEEPS * ACTION_SIZE * ACTION_SIZE;
    }

    public class HierarchicalDQN
    {
        private DQN sourceKeepDQN;
        private DQN targetKeepDQN;
        private DQN soldierAllocationDQN;
        private DQN archerAllocationDQN;
        private int nKeeps;

        public HierarchicalDQN(int nKeeps)
        {
            this.nKeeps = nKeeps;
            sourceKeepDQN = new DQN(nKeeps, nKeeps, "SourceKeepDQN");
            targetKeepDQN = new DQN(nKeeps, nKeeps, "TargetKeepDQN");
            soldierAllocationDQN = new DQN(nKeeps, Constants.ACTION_SIZE, "SoldierAllocationDQN");
            archerAllocationDQN = new DQN(nKeeps, Constants.ACTION_SIZE, "ArcherAllocationDQN");
        }

        public Action GetAction(GameState gameState, float epsilon, int ownAlliance)
        {
            if (new Random().NextDouble() < epsilon)
            {
                // Epsilon-greedy exploration
                return new Action
                {
                    SourceKeep = new Random().Next(nKeeps),
                    TargetKeep = new Random().Next(nKeeps),
                    SoldierPercent = new Random().Next(Constants.ACTION_SIZE),
                    ArcherPercent = new Random().Next(Constants.ACTION_SIZE)
                };
            }
            else
            {
                int sourceKeep = sourceKeepDQN.GetAction(gameState, ownAlliance);
                int targetKeep = targetKeepDQN.GetAction(gameState, ownAlliance);
                int soldierPercent = soldierAllocationDQN.GetAction(gameState, ownAlliance);
                int archerPercent = archerAllocationDQN.GetAction(gameState, ownAlliance);

                return new Action
                {
                    SourceKeep = sourceKeep,
                    TargetKeep = targetKeep,
                    SoldierPercent = soldierPercent,
                    ArcherPercent = archerPercent
                };
            }
        }

        public void Train(GameState state, Action action, float reward, GameState nextState, bool done, int ownAlliance)
        {
            sourceKeepDQN.Train(state, action.SourceKeep, reward, nextState, done, ownAlliance);
            targetKeepDQN.Train(state, action.TargetKeep, reward, nextState, done, ownAlliance);
            soldierAllocationDQN.Train(state, action.SoldierPercent, reward, nextState, done, ownAlliance);
            archerAllocationDQN.Train(state, action.ArcherPercent, reward, nextState, done, ownAlliance);
        }
    }

    public class DQN
    {
        private MLContext mlContext;
        private ITransformer model;
        private ITransformer targetModel;
        private int actionSpace;
        private int nKeeps;
        private string name;

        public DQN(int nKeeps, int actionSpace, string name)
        {
            this.nKeeps = nKeeps;
            this.actionSpace = actionSpace;
            this.name = name;
            mlContext = new MLContext();
            model = BuildModel();
            targetModel = BuildModel();
            UpdateTarget();
        }

        private ITransformer BuildModel()
        {
            // Define the TensorFlow model
            var pipeline = mlContext.Model.LoadTensorFlowModel($"{name}_model.pb")
                .ScoreTensorFlowModel(
                    inputColumnNames: new[] { "MapState", "KeepOwnership", "SoldierCounts", "ArcherCounts", "OwnAlliance" },
                    outputColumnNames: new[] { "QValues" },
                    addBatchDimensionInput: true);

            // Create an empty DataView to train the model
            var emptyData = mlContext.Data.LoadFromEnumerable(new List<InputData>());
            return pipeline.Fit(emptyData);
        }

        public void UpdateTarget()
        {
            targetModel = model;
        }

        public int GetAction(GameState gameState, int ownAlliance)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<InputData, OutputData>(model);
            var prediction = predictionEngine.Predict(new InputData
            {
                MapState = gameState.MapState,
                KeepOwnership = gameState.KeepOwnership,
                SoldierCounts = gameState.SoldierCounts,
                ArcherCounts = gameState.ArcherCounts,
                OwnAlliance = ownAlliance
            });

            return np.argmax(prediction.QValues);
        }

        public void Train(GameState state, int action, float reward, GameState nextState, bool done, int ownAlliance)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<InputData, OutputData>(model);
            var targetPredictionEngine = mlContext.Model.CreatePredictionEngine<InputData, OutputData>(targetModel);

            OutputData prediction = predictionEngine.Predict(new InputData
            {
                MapState = state.MapState,
                KeepOwnership = state.KeepOwnership,
                SoldierCounts = state.SoldierCounts,
                ArcherCounts = state.ArcherCounts,
                OwnAlliance = ownAlliance
            });

            float targetQ;
            if (done)
            {
                targetQ = reward;
            }
            else
            {
                var nextPrediction = targetPredictionEngine.Predict(new InputData
                {
                    MapState = nextState.MapState,
                    KeepOwnership = nextState.KeepOwnership,
                    SoldierCounts = nextState.SoldierCounts,
                    ArcherCounts = nextState.ArcherCounts,
                    OwnAlliance = ownAlliance
                });
                float maxQ = np.max(nextPrediction.QValues);
                targetQ = reward + 0.99f * maxQ;
            }

            prediction.QValues[action] = (1 - 0.1f) * prediction.QValues[action] + 0.1f * targetQ;

            // Here you would update the model with the new target
            // This is a simplified representation and would need to be implemented
            // based on how you're handling model updates in ML.NET
        }
    }

    public class InputData
    {
        [VectorType(Constants.MAP_SIZE, Constants.MAP_SIZE, Constants.NUM_TILES)]
        public float[] MapState { get; set; }

        [VectorType(Constants.NUM_KEEPS)]
        public float[] KeepOwnership { get; set; }

        [VectorType(Constants.NUM_KEEPS)]
        public float[] SoldierCounts { get; set; }

        [VectorType(Constants.NUM_KEEPS)]
        public float[] ArcherCounts { get; set; }

        public int OwnAlliance { get; set; }
    }

    public class OutputData
    {
        [VectorType(-1)]  // Length will be set based on the specific DQN's action space
        public float[] QValues { get; set; }
    }

    public class GameState
    {
        public float[] MapState { get; set; }
        public float[] KeepOwnership { get; set; }
        public float[] SoldierCounts { get; set; }
        public float[] ArcherCounts { get; set; }
    }

    public class Action
    {
        public int SourceKeep { get; set; }
        public int TargetKeep { get; set; }
        public int SoldierPercent { get; set; }
        public int ArcherPercent { get; set; }
    }
}