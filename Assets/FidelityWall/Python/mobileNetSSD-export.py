from huggingface_hub import hf_hub_download

onnx_file_path = hf_hub_download(
    repo_id="onnxmodelzoo/ssd_mobilenet_v1_12", filename="mobileNetSSD.onnx"
)
print("Downloaded to:", onnx_file_path)
