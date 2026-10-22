import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import DataLoader, TensorDataset
import numpy as np

# 1. Load data from separate comma-delimited text files
# pixels.txt: flat row of 190 values per image
# labels.txt: a single integer (0-9) per line
try:
    X_raw = np.loadtxt('pixels.txt', delimiter=',')
    y_raw = np.loadtxt('labels.txt', delimiter=',', dtype=np.int64)
except FileNotFoundError:
    print("Files not found. Generating dummy text data for demonstration...")
    np.savetxt('pixels.txt', np.random.rand(100, 190), delimiter=',')
    np.savetxt('labels.txt', np.random.randint(0, 10, 100), delimiter=',', fmt='%d')
    X_raw = np.loadtxt('pixels.txt', delimiter=',')
    y_raw = np.loadtxt('labels.txt', delimiter=',', dtype=np.int64)

# Convert NumPy arrays to PyTorch Tensors
X_tensor = torch.tensor(X_raw, dtype=torch.float32)
y_tensor = torch.tensor(y_raw, dtype=torch.long)

# Create a DataLoader for batch processing
dataset = TensorDataset(X_tensor, y_tensor)
dataloader = DataLoader(dataset, batch_size=16, shuffle=True)

# 2. Define the Neural Network for 10 classifications
class ImageClassifier(nn.Module):
    def __init__(self):
        super(ImageClassifier, self).__init__()
        self.network = nn.Sequential(
            nn.Linear(19 * 10, 64),  # Input layer (19x10 = 190 flat pixels) 14,634 parameters total
            nn.ReLU(),
            nn.Linear(64, 32),       # Hidden layer 
            nn.ReLU(),
            nn.Linear(32, 10)        # 10 classifications output layer
        )
        
    def forward(self, x):
        return self.network(x)

model = ImageClassifier()

total_params = sum(p.numel() for p in model.parameters())
print(f"Total parameters: {total_params:,}")


# 3. Train the model
criterion = nn.CrossEntropyLoss()
optimizer = optim.Adam(model.parameters(), lr=0.001)

model.train()
epochs = 500 #100
for epoch in range(epochs):
    epoch_loss = 0.0
    for batch_X, batch_y in dataloader:
        optimizer.zero_grad()
        outputs = model(batch_X)
        loss = criterion(outputs, batch_y)
        loss.backward()
        optimizer.step()
        epoch_loss += loss.item()
        
    print(f"Epoch {epoch+1}/{epochs} | Loss: {epoch_loss/len(dataloader):.4f}")

# 4. Export the trained model to ONNX format
model.eval() # Set model to evaluation mode

# Create a mock input matching the input shape (1 sample, 190 pixels)
dummy_input = torch.randn(1, 19 * 10) 

# Export the graph
onnx_filename = "fidelity.onnx"
torch.onnx.export(
    model, 
    dummy_input, 
    onnx_filename, 
    export_params=True,        # Store the trained parameter weights inside the model
    opset_version=11,          # ONNX version to export to
    input_names=['input'],     # Name the input tensor
    output_names=['output'],   # Name the output tensor
    dynamic_axes={             # Enable dynamic batch sizing for flexible inference
        'input': {0: 'batch_size'}, 
        'output': {0: 'batch_size'}
    }
)

print(f"\nSuccess! Model trained and exported to: {onnx_filename}")
