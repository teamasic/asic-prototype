import os
import random
import shutil
from pathlib import Path

from imutils import paths
from sklearn import datasets

fullDatasetDir = "dataset"
testDir = "images"
trainingDir = "training"
augmentedDir = "augmented"


# remove old data
def copyFolderToTest():
    if os.path.exists(trainingDir):
        shutil.rmtree(trainingDir)
    if os.path.exists(testDir):
        shutil.rmtree(testDir)
    if os.path.exists(augmentedDir):
        shutil.rmtree(augmentedDir)
    # start loop
    for (rootDir, subDirs, files) in os.walk(fullDatasetDir):
        for subDatasetDir in subDirs:
            numOfTrain = 4
            numOfTest = None
            if (subDatasetDir == "unknown"):
                numOfTrain = 450
                numOfTest = 50
            # Get necessary paths
            fullPathSubDatasetDir = os.path.join(fullDatasetDir, subDatasetDir)
            print(fullPathSubDatasetDir)
            fullPathSubTrainingDir = os.path.join(trainingDir, subDatasetDir)
            fullPathSubTestingDir = os.path.join(testDir, subDatasetDir)

            # Get random some image from each face to test and remain for training
            imagePaths = list(paths.list_images(fullPathSubDatasetDir))
            imagePathsToTrain = random.sample(imagePaths, numOfTrain)
            if numOfTest is None:
                imagePathsToTest = [x for x in imagePaths if x not in imagePathsToTrain]
            else:
                imagePathsToTest = random.sample([x for x in imagePaths if x not in imagePathsToTrain], numOfTest)

            for imagePath in imagePathsToTest:
                Path(fullPathSubTestingDir).mkdir(parents=True, exist_ok=True)
                shutil.copy(imagePath, fullPathSubTestingDir)

            for imagePath in imagePathsToTrain:
                Path(fullPathSubTrainingDir).mkdir(parents=True, exist_ok=True)
                shutil.copy(imagePath, fullPathSubTrainingDir)


# copyFolderToTest()
# os.system("python augment.py --dataset training")
os.system("python extract_embeddings.py --dataset augmented")
# os.system("python train_model.py")
# os.system("python test_result.py")
