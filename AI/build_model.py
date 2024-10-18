import tensorflow as tf
import numpy as np

def create_model(input_shape, n_keeps):
    model = tf.keras.Sequential([
        tf.keras.layers.Input(shape=input_shape),
        
        # Convolutional layers for processing the map
        tf.keras.layers.Conv2D(32, (3, 3), activation='relu'),
        tf.keras.layers.MaxPooling2D((2, 2)),
        tf.keras.layers.Conv2D(64, (3, 3), activation='relu'),
        tf.keras.layers.MaxPooling2D((2, 2)),
        tf.keras.layers.Flatten(),
        
        # Dense layers for processing flattened map and additional game state
        tf.keras.layers.Dense(256, activation='relu'),
        tf.keras.layers.Dense(128, activation='relu'),
        
        # Output layers for each action component
        tf.keras.layers.Dense(n_keeps, name='source_keep'),
        tf.keras.layers.Dense(n_keeps, name='target_keep'),
        tf.keras.layers.Dense(4, name='soldier_percent'),  # 0, 25, 50, 75 percent
        tf.keras.layers.Dense(4, name='archer_percent')    # 0, 25, 50, 75 percent
    ])
    
    return model

# Define your input shape and action space parameters
input_shape = (256, 256, 1)  # Adjust based on your game state
n_keeps = 64  # Adjust based on your game setup

# Create the model
model = create_model(input_shape, n_keeps)

# Custom loss function to handle multi-part action
def multi_action_loss(y_true, y_pred):
    # Assuming y_true and y_pred are lists of 4 tensors each
    source_loss = tf.keras.losses.sparse_categorical_crossentropy(y_true[0], y_pred[0])
    target_loss = tf.keras.losses.sparse_categorical_crossentropy(y_true[1], y_pred[1])
    soldier_loss = tf.keras.losses.sparse_categorical_crossentropy(y_true[2], y_pred[2])
    archer_loss = tf.keras.losses.sparse_categorical_crossentropy(y_true[3], y_pred[3])
    return source_loss + target_loss + soldier_loss + archer_loss

# Compile the model
model.compile(optimizer='adam', loss=multi_action_loss)

# Save the model in SavedModel format
tf.saved_model.save(model, "AI/saved_model")

# Convert SavedModel to frozen graph
from tensorflow.python.framework.convert_to_constants import convert_variables_to_constants_v2

# Convert Keras model to ConcreteFunction
full_model = tf.function(lambda x: model(x))
full_model = full_model.get_concrete_function(
    tf.TensorSpec(model.inputs[0].shape, model.inputs[0].dtype))

# Get frozen ConcreteFunction
frozen_func = convert_variables_to_constants_v2(full_model)
frozen_func.graph.as_graph_def()

# Save frozen graph from frozen ConcreteFunction to hard drive
tf.io.write_graph(graph_or_graph_def=frozen_func.graph,
                  logdir="./AI/",
                  name="model.pb",
                  as_text=False)

print("Model saved successfully as model.pb")

# Print input and output names
print("Input node name:", model.inputs[0].name)
for output in model.outputs:
    print("Output node name:", output.name)