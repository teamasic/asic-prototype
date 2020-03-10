import os
import shutil
from pathlib import Path

from imutils import paths

datasetDir = "E:/capstone/asian/Asian"
resultDir = "E:/capstone/total_dataset/asian_big"
listPeople = {}
imagePaths = paths.list_images(datasetDir)

for imagePath in imagePaths:
    imageName = imagePath.split('\\')[1]
    personName = imageName.split("_")[0]
    personPose = imageName.split("_")[2]
    if personPose.find("90") == -1 and personPose.find("60") == -1:
        if (personName not in listPeople):
            listImages = [imagePath]
            listPeople[personName] = listImages
        else:
            listImages = listPeople[personName]
            listImages.append(imagePath)
            listPeople[personName] = listImages
for key, value in listPeople.items():
    Path(os.path.join(resultDir, key)).mkdir(parents=True, exist_ok=True)
    for imagePath in value:
        shutil.copy(imagePath, os.path.join(resultDir, key))
