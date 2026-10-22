import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import Dataset, DataLoader
import numpy as np

model = ""
dummy_input = ""

# 1. Custom Dataset for Text-based MNIST Data
class MNISTTextDataset(Dataset):

  def __init__(
      self, image_file, label_file, delimiter=",", transform=None
  ):
    print(f"Loading images from {image_file}...")
    # Load pixel values: shape (N, 784)
    self.images = np.loadtxt(image_file, delimiter=delimiter, dtype=np.float32)
    # Normalize pixel values from 0-255 to 0.0-1.0
#    self.images = self.images / 255.0
    # Reshape from (N, 784) to (N, 1, 28, 28) for neural networks
    self.images = self.images.reshape(-1, 1, 19, 10)

    print(f"Loading labels from {label_file}...")
    # Load integer classification labels: shape (N,)
    self.labels = np.loadtxt(label_file, delimiter=delimiter, dtype=np.int64)
    self.transform = transform
    print(f"done...")

  def __len__(self):
    return len(self.labels)

  def __getitem__(self, idx):
    image = torch.tensor(self.images[idx], dtype=torch.float32)
    label = torch.tensor(self.labels[idx], dtype=torch.long)
    return image, label


# 2. Simple Neural Network Architecture (MLP)
class MNISTClassifier(nn.Module):

  def __init__(self):
    super(MNISTClassifier, self).__init__()
    self.flatten = nn.Flatten()
    self.fc_seq = nn.Sequential(
        nn.Linear(190, 128),   # 19 x 10 from 1920x1080
        nn.ReLU(),
        nn.Linear(128, 64),
        nn.ReLU(),
        nn.Linear(64, 10),  # 10 output classes (digits 0-9, -1)
    )

  def forward(self, x):
    x = self.flatten(x)
    return self.fc_seq(x)


# 3. Training Loop
def train_mnist_from_txt():
  # File paths (adjust to your local text files)
  train_img_path = "pixels.txt"
  train_lbl_path = "classifications.txt"

  # Check device (GPU if available, else CPU)
  device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
  print(f"Training on device: {device}")

  # Create Dataset and DataLoader
  train_dataset = MNISTTextDataset(
      image_file=train_img_path,
      label_file=train_lbl_path,
      delimiter=",",  # Change to ',' if your files are comma-separated
  )

  train_loader = DataLoader(train_dataset, batch_size=64, shuffle=True)

  # Initialize model, loss function, and optimizer
  model = MNISTClassifier().to(device)
  criterion = nn.CrossEntropyLoss()
  optimizer = optim.Adam(model.parameters(), lr=0.001)

  # Training epochs
  epochs = 5
  model.train()
  for epoch in range(epochs):
    running_loss = 0.0
    correct = 0
    total = 0
    print("start")
    for images, labels in train_loader:
      images, labels = images.to(device), labels.to(device)

      optimizer.zero_grad()
      outputs = model(images)
      loss = criterion(outputs, labels)
      loss.backward()
      optimizer.step()

      running_loss += loss.item() * images.size(0)
      _, predicted = torch.max(outputs.data, 1)
      total += labels.size(0)
      correct += (predicted == labels).sum().item()
      print("start.")

    epoch_loss = running_loss / len(train_dataset)
    epoch_acc = 100.0 * correct / total
    print(
        f"Epoch [{epoch+1}/{epochs}] - Loss: {epoch_loss:.4f} -"
        f" Accuracy: {epoch_acc:.2f}%"
    )

  # Save trained model
  torch.save(model.state_dict(), "fidelity.pth")
  print("Model saved to mnist_text_model.pth")


if __name__ == "__main__":
  train_mnist_from_txt()
  torch.onnx.export(
    model,
    dummy_input,
    "fidelity.onnx",
    export_params=True,
    opset_version=11,
    do_constant_folding=True,
    input_names=["input"],
    output_names=["output"],
    dynamic_axes={"input": {0: "batch_size"}, "output": {0: "batch_size"}},
   )
