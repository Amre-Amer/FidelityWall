from ultralytics import YOLO

# Load your model (nano version is recommended for real-time game engines)
model = YOLO("yolov8n.pt") 

# Export the model to ONNX format with a fixed 640x640 shape
model.export(format="onnx", imgsz=640, opset=12)
