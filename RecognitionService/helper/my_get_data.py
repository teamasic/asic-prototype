import os
import shutil
from pathlib import Path

from imutils import paths


def generate_image_asian(asianDatasetDir, resultDir):
    listPeople = {}
    imagePaths = list(paths.list_images(asianDatasetDir))

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


def get_unknown_images(datasetDir, resultDir):
    listImages = []
    totalImage = 500
    for (root, subDirs, files) in os.walk(datasetDir):
        for subDir in subDirs:
            fullSubDir = os.path.join(datasetDir, subDir)
            image = paths.list_images(fullSubDir).__next__()
            if (len(listImages) < totalImage):
                listImages.append(image)
                shutil.copy(image, resultDir)
            else:
                break
        break
    print(len(listImages))
