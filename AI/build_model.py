import tensorflow as tf
import numpy as np


def create_model(input_shape, n_keeps, n_players):
    # Main input: game map
    map_input = tf.keras.layers.Input(shape=input_shape, name="map_input")

    # Keep ownership input
    keep_ownership = tf.keras.layers.Input(
        shape=(n_keeps,), name="keep_ownership", dtype="int32"
    )

    # Unit count inputs
    soldier_counts = tf.keras.layers.Input(shape=(n_keeps,), name="soldier_counts")
    archer_counts = tf.keras.layers.Input(shape=(n_keeps,), name="archer_counts")

    # Process the map
    x = tf.keras.layers.Conv2D(32, (3, 3), activation="relu")(map_input)
    x = tf.keras.layers.MaxPooling2D((2, 2))(x)
    x = tf.keras.layers.Conv2D(64, (3, 3), activation="relu")(x)
    x = tf.keras.layers.MaxPooling2D((2, 2))(x)
    x = tf.keras.layers.Flatten()(x)

    # Process keep ownership
    ownership_embedding = tf.keras.layers.Embedding(n_players + 1, 8)(
        keep_ownership
    )  # +1 for potentially unowned keeps
    ownership_flatten = tf.keras.layers.Flatten()(ownership_embedding)

    # Combine all keep information
    keep_info = tf.keras.layers.Concatenate()(
        [ownership_flatten, soldier_counts, archer_counts]
    )
    keep_info = tf.keras.layers.Dense(64, activation="relu")(keep_info)

    # Combine map and keep information
    combined = tf.keras.layers.Concatenate()([x, keep_info])

    # Dense layers for processing combined information
    x = tf.keras.layers.Dense(256, activation="relu")(combined)
    x = tf.keras.layers.Dense(128, activation="relu")(x)

    # Output layers for each action component
    source_keep = tf.keras.layers.Dense(
        n_keeps, name="source_keep", activation="softmax"
    )(x)
    target_keep = tf.keras.layers.Dense(
        n_keeps, name="target_keep", activation="softmax"
    )(x)
    soldier_percent = tf.keras.layers.Dense(
        4, name="soldier_percent", activation="softmax"
    )(x)
    archer_percent = tf.keras.layers.Dense(
        4, name="archer_percent", activation="softmax"
    )(x)

    model = tf.keras.Model(
        inputs=[map_input, keep_ownership, soldier_counts, archer_counts],
        outputs=[source_keep, target_keep, soldier_percent, archer_percent],
    )

    return model


# Define your input shape and action space parameters
input_shape = (256, 256, 1)  # Adjust based on your game state
n_keeps = 32  # Adjust based on your game setup
n_players = 20

# Create the model
model = create_model(input_shape, n_keeps, n_players)


# Custom loss function to handle multi-part action
def multi_action_loss(y_true, y_pred):
    # Assuming y_true and y_pred are lists of 4 tensors each
    source_loss = tf.keras.losses.sparse_categorical_crossentropy(y_true[0], y_pred[0])
    target_loss = tf.keras.losses.sparse_categorical_crossentropy(y_true[1], y_pred[1])
    soldier_loss = tf.keras.losses.sparse_categorical_crossentropy(y_true[2], y_pred[2])
    archer_loss = tf.keras.losses.sparse_categorical_crossentropy(y_true[3], y_pred[3])
    return source_loss + target_loss + soldier_loss + archer_loss


# Compile the model
model.compile(optimizer="adam", loss=multi_action_loss)

# Save the model in SavedModel format
tf.saved_model.save(model, "AI/saved_model")

# Convert SavedModel to frozen graph
from tensorflow.python.framework.convert_to_constants import (
    convert_variables_to_constants_v2,
)


# Create concrete function
@tf.function(
    input_signature=[
        tf.TensorSpec(shape=(None, 256, 256, 1), dtype=tf.float32, name="map_input"),
        tf.TensorSpec(shape=(None, n_keeps), dtype=tf.int32, name="keep_ownership"),
        tf.TensorSpec(shape=(None, n_keeps), dtype=tf.float32, name="soldier_counts"),
        tf.TensorSpec(shape=(None, n_keeps), dtype=tf.float32, name="archer_counts"),
    ]
)
def serving_function(map_input, keep_ownership, soldier_counts, archer_counts):
    return model([map_input, keep_ownership, soldier_counts, archer_counts])


# Get concrete function
concrete_function = serving_function.get_concrete_function()

# Convert to frozen graph
frozen_func = convert_variables_to_constants_v2(concrete_function)
frozen_func.graph.as_graph_def()

# Save frozen graph from frozen ConcreteFunction to hard drive
tf.io.write_graph(
    graph_or_graph_def=frozen_func.graph, logdir="./AI/", name="model.pb", as_text=False
)

print("Model saved successfully as model.pb")

# Print input and output names
print("Input node name:", model.inputs[0].name)
for output in model.outputs:
    print("Output node name:", output.name)
