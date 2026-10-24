from ultralytics import YOLO

# 1. Load a standard pretrained model or a blank structure
# Setting ch=1 configures the architecture for 1-channel grayscale inputs
# model = YOLO("yolov8n.yaml")  # Load structure
model = YOLO("yolov8n.pt")  # Load structure
model.model.args = dict(ch=1) # Force 1-channel input if training from scratch

# 2. Train the model
# imgsz is passed as (height, width) -> (20, 38)
results = model.train(
    data="yolov8n.pt",  # data="dataset.yaml", 
    epochs=100, 
    imgsz=(20, 38), 
    batch=16
)

# 3. Run Inference on a grayscale image
#metrics = model("path/to/grayscale_image.jpg", imgsz=(20, 38))
