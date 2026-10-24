import onnx

model = onnx.load("yolov8n.onnx")
inputs = model.graph.input

for input in inputs:
    # Dimension format is typically [Batch, Channels, Height, Width]
    dimensions = [dim.dim_value for dim in input.type.tensor_type.shape.dim]
    print(f"Input Name: {input.name} | Shape: {dimensions}") 
    # Example Output: [1, 3, 640, 640]
