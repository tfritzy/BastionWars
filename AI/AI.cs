using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
using NumSharp;

namespace KeepLordWarriors.AI;

public class DQN
{
    private MLContext mlContext;
    private ITransformer model;
    private ITransformer targetModel;
    private int actionSpace;
    private int[] stateShape;

    public DQN(int[] stateShape, int actionSpace)
    {
        this.stateShape = stateShape;
        this.actionSpace = actionSpace;
        mlContext = new MLContext();
        model = BuildModel();
        targetModel = BuildModel();
        UpdateTarget();
    }

    private ITransformer BuildModel()
    {
        // Define the TensorFlow model
        var pipeline = mlContext.Model.LoadTensorFlowModel("model.pb")
            .ScoreTensorFlowModel(
                inputColumnNames: new[] { "input_1" },
                outputColumnNames: new[] { "dense_2" },
                addBatchDimensionInput: true);

        // Create an empty DataView to train the model
        var emptyData = mlContext.Data.LoadFromEnumerable(new List<InputData>());
        return pipeline.Fit(emptyData);
    }

    public void UpdateTarget()
    {
        targetModel = model;
    }

    public (int sourceKeep, int targetKeep, int soldierPercent, int archerPercent) GetAction(float[] state, float epsilon)
    {
        if (new Random().NextDouble() < epsilon)
        {
            // Epsilon-greedy exploration
            return (
                new Random().Next(Constants.NUM_KEEPS),
                new Random().Next(Constants.NUM_KEEPS),
                new Random().Next(Constants.ACTION_SIZE),
                new Random().Next(Constants.ACTION_SIZE)
            );
        }
        else
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<InputData, OutputData>(model);
            var prediction = predictionEngine.Predict(new InputData { Input = state });

            return (
                np.argmax(prediction.SourceKeep),
                np.argmax(prediction.TargetKeep),
                np.argmax(prediction.SoldierPercent),
                np.argmax(prediction.ArcherPercent)
            );
        }
    }

    public void Train(float[] state, (int sourceKeep, int targetKeep, int soldierPercent, int archerPercent) action, float reward, float[] nextState, bool done)
    {
        var predictionEngine = mlContext.Model.CreatePredictionEngine<InputData, OutputData>(model);
        var targetPredictionEngine = mlContext.Model.CreatePredictionEngine<InputData, OutputData>(targetModel);

        var prediction = predictionEngine.Predict(new InputData { Input = state });

        if (done)
        {
            // Update the Q-value for the taken action
            // This is a simplified approach and may need to be adjusted based on your specific requirements
            prediction.SourceKeep[action.sourceKeep] = reward;
            prediction.TargetKeep[action.targetKeep] = reward;
            prediction.SoldierPercent[action.soldierPercent] = reward;
            prediction.ArcherPercent[action.archerPercent] = reward;
        }
        else
        {
            var nextPrediction = targetPredictionEngine.Predict(new InputData { Input = nextState });
            float maxQ = Math.Max(
                Math.Max(np.max(nextPrediction.SourceKeep), np.max(nextPrediction.TargetKeep)),
                Math.Max(np.max(nextPrediction.SoldierPercent), np.max(nextPrediction.ArcherPercent))
            );

            float newQ = reward + 0.99f * maxQ;

            prediction.SourceKeep[action.sourceKeep] = newQ;
            prediction.TargetKeep[action.targetKeep] = newQ;
            prediction.SoldierPercent[action.soldierPercent] = newQ;
            prediction.ArcherPercent[action.archerPercent] = newQ;
        }

        // Here you would update the model with the new target
        // This is a simplified representation and would need to be implemented
        // based on how you're handling model updates in ML.NET
    }
}

public class InputData
{
    [VectorType(Constants.MAP_SIZE, Constants.MAP_SIZE, Constants.NUM_TILES)]
    public float[] Input { get; set; }
}

public class OutputData
{
    [VectorType(Constants.NUM_KEEPS)]
    public float[] SourceKeep { get; set; }

    [VectorType(Constants.NUM_KEEPS)]
    public float[] TargetKeep { get; set; }

    [VectorType(Constants.ACTION_SIZE)]
    public float[] SoldierPercent { get; set; }

    [VectorType(Constants.ACTION_SIZE)]
    public float[] ArcherPercent { get; set; }
}

// Main training loop
public class Trainer
{
    public void Train()
    {
        var game = new KeepLordWarriors.Game(new Schema.GameSettings());
        var dqn = new DQN(
            new[] { Constants.MAP_SIZE, Constants.MAP_SIZE, Constants.NUM_TILES },
            Constants.TOTAL_ACTION_SPACE);
        float epsilon = 1.0f;
        float epsilonMin = 0.01f;
        float epsilonDecay = 0.995f;

        for (int episode = 0; episode < 10000; episode++)
        {
            float[] state = game.Reset();
            bool done = false;
            while (!done)
            {
                var action = dqn.GetAction(state, epsilon);
                var (nextState, reward, isDone) = game.Step(action);
                dqn.Train(state, action, reward, nextState, isDone);
                state = nextState;
                done = isDone;
            }

            epsilon = Math.Max(epsilonMin, epsilon * epsilonDecay);

            if (episode % 100 == 0)
            {
                dqn.UpdateTarget();
            }
        }
    }
}