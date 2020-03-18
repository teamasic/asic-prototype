import os
import shutil
from pathlib import Path
import random

import cv2
from imutils import paths

from helper import my_face_detection, my_service

fullDatasetDir = "dataset"
testDir = "images"
trainingDir = "training"
augmentedDir = "augmented"
outputDlibDir = "output_dlib"

def test_detect_full(dir, removed=False):
    imagePaths = list(paths.list_images(dir))
    listFailed = []
    failedDetectDir = "temp/failed_detect"
    for (i, imagePath) in enumerate(imagePaths):
        print("Try detect image {}/{}".format(i, len(imagePaths)))
        image = cv2.imread(imagePath)
        boxes = my_face_detection.face_locations(image)
        if len(boxes) != 1:
            print("FALSE - {} - Length: {}".format(imagePath, len(boxes)))
            listFailed.append(imagePath)

            Path(failedDetectDir).mkdir(parents=True, exist_ok=True)
            shutil.copy(imagePath, failedDetectDir)

            if (removed is True):
                os.remove(imagePath)
    print("Failed {}/{}".format(len(listFailed), len(imagePaths)))


def test_detect_image(path, removed=False):
    failedDetectDir = "temp/failed_detect"
    image = cv2.imread(path)
    boxes = my_face_detection.face_locations(image)
    if len(boxes) != 1:
        print("FALSE - {} - Length: {}".format(path, len(boxes)))

        Path(failedDetectDir).mkdir(parents=True, exist_ok=True)
        shutil.copy(path, failedDetectDir)

        if (removed is True):
            os.remove(path)

        return False
    return True


def split_train_test(numOfTrain, numOfTest, numOfTrainUnknown=450, numOfTestUnknown=50):

    if os.path.exists(trainingDir):
        shutil.rmtree(trainingDir)
    if os.path.exists(testDir):
        shutil.rmtree(testDir)
    if os.path.exists(augmentedDir):
        shutil.rmtree(augmentedDir)
    if not os.path.exists(outputDlibDir):
        os.mkdir(outputDlibDir)
    # start loop
    for (rootDir, subDirs, files) in os.walk(fullDatasetDir):
        for subDatasetDir in subDirs:
            if (subDatasetDir == "unknown"):
                numOfTrain = numOfTrainUnknown
                numOfTest = numOfTestUnknown
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

def test_result():
    imagePathsToTest = list(paths.list_images(testDir))

    # Tổng số hình test mà có người test có trong db
    totalInDb = 0
    truePositive = 0

    # Tổng số hình test mà có người test không có trong db
    totalNotInDb = 0
    trueNegative = 0
    for imagePath in imagePathsToTest:
        print(imagePath)
        try:
            resultName = imagePath.split(os.path.sep)[-2]
            box, calculateName, proba = my_service.recognize_image(imagePath)
            print(proba)
            print(resultName + "-" + calculateName)
        except:
            continue
        if resultName == "unknown":
            totalNotInDb += 1
            if resultName == calculateName:
                trueNegative += 1
        else:
            totalInDb += 1
            if resultName == calculateName:
                truePositive += 1
    falseNegative = totalInDb - truePositive
    falsePositive = totalNotInDb - trueNegative
    print("Total true positive = {}", truePositive)
    print("Total false negative = {}", totalInDb - truePositive)
    print("Total true negative = {}", trueNegative)
    print("Total false positive = {}", totalNotInDb - trueNegative)

    print("Sensitivity [TP / (TP + FN)] = {}", truePositive / totalInDb)
    print("Precision [TP / (TP + FP)] = {}", truePositive / (truePositive + falsePositive))
    print("Accuracy [(TP + TN) / (TP + FP + TN + FN)] = {}", (truePositive + trueNegative) / (totalInDb + totalNotInDb))

def copyUnknownImages():
    unknownImagesPath = "unknow_data/images/unknown"
    unknownImagesPathReal = "images/unknown"
    Path(unknownImagesPathReal).mkdir(parents=True, exist_ok=True)
    listImages = list(paths.list_images(unknownImagesPath))
    for image in listImages:
        shutil.copy(image, unknownImagesPathReal)

def getImagesNotInTraining():
    for (rootDir, subDirs, files) in os.walk(fullDatasetDir):
        for subDatasetDir in subDirs:
            fullPathSubDatasetDir = os.path.join(fullDatasetDir, subDatasetDir)
            fullPathSubTrainDir = os.path.join(trainingDir, subDatasetDir)
            fullPathSubTestDir = os.path.join(testDir, subDatasetDir)
            imagePaths = list(paths.list_images(fullPathSubDatasetDir))
            imagePathsTrain = list(paths.list_images(fullPathSubTrainDir))
            shutil.rmtree(fullPathSubTestDir)
            Path(fullPathSubTestDir).mkdir(parents=True, exist_ok=True)
            imagePathsWithoutRoot = []
            imagePathsTrainWithoutRoot = []
            for imagePath in imagePaths:
                imagePathTemp = os.path.join(imagePath.split("\\")[1], imagePath.split("\\")[2])
                imagePathsWithoutRoot.append(imagePathTemp)
            for imagePath in imagePathsTrain:
                imagePathTemp = os.path.join(imagePath.split("\\")[1], imagePath.split("\\")[2])
                imagePathsTrainWithoutRoot.append(imagePathTemp)
            imagesAdd = [x for x in imagePathsWithoutRoot if x not in imagePathsTrainWithoutRoot]
            fullImageAddPath = list(map(lambda x: os.path.join(fullDatasetDir, x), imagesAdd))
            print(fullImageAddPath)
            for image in fullImageAddPath:
                shutil.copy(image, fullPathSubTestDir)
