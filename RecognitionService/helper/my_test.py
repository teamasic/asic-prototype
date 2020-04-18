import os
import pickle
import shutil
from pathlib import Path
import random

import cv2
import imutils
from imutils import paths
import numpy as np
import matplotlib.pyplot as plt

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
            print("Sub", subDatasetDir)
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


def getData(result, desiredType):
    desiredResult = []
    for (resultName, calculateName, proba, type) in result:
        if desiredType == type:
            desiredResult.append((resultName, calculateName, proba, type))
    return desiredResult


def getThresholdData(minThreshold, maxThreshold, step, result):
    myData = {}
    indbLengh = len([x for x in result if (x[3] == "TP" or x[3] == "FN1" or x[3] == "FN2")])
    for a in range(int(minThreshold * 1000), int((maxThreshold + step) * 1000), int(step * 1000)):
        i = a/1000
        fn1count = [x for x in result if (x[3] == "FN1" and x[2] >= i)]
        fn2count = [x for x in result if (x[3] == "FN2" or (x[3] == "FN1" and x[2] < i) or (x[3] == "TP" and x[2] < i))]
        myData[i] = (len(fn1count) / indbLengh, len(fn2count) / indbLengh, (indbLengh - len(fn1count) - len(fn2count))/indbLengh)
        myData[i] = [x * 100 for x in myData[i]]
    return myData


def drawChart(thresholdData):
    threshold = []
    fn1 = []
    fn2 = []
    for key, value in thresholdData.items():
        threshold.append(str(key))
        fn1.append(value[0])
        fn2.append(value[1])
    index = np.arange(len(thresholdData.keys()))
    width = 0.3
    plt.bar(index, fn1, width, color="blue", label="Recognition wrong")
    plt.bar(index, fn2, width, color="red", label="Cannot recognition", bottom= fn1)
    plt.title = "Change in failed recognition based on threshold"
    plt.xlabel("Threshold")
    plt.ylabel("Failed recognition rate")
    plt.xticks(index, threshold)

    for i, value in enumerate(fn1):
        plt.text(i, round(value, 2), str(round(value, 2)))
    for i, value in enumerate(fn1):
        plt.text(i, round(value + fn2[i], 2), str(round(value + fn2[i], 2)))

    plt.legend(loc="best")
    plt.show()



def test_result():
    result = []

    imagePathsToTest = list(paths.list_images(testDir))

    # Tổng số hình test mà có người test có trsong db
    totalInDb = 0
    truePositive = 0

    falseNegative1 = 0
    falseNegative2 = 0

    # Tổng số hình test mà có người test không có trong db
    totalNotInDb = 0
    trueNegative = 0
    for imagePath in imagePathsToTest:
        print(imagePath)
        try:
            resultName = imagePath.split(os.path.sep)[-2]
            image = cv2.imread(imagePath)
            image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
            image = imutils.resize(image, width=400)
            box, calculateName, proba = my_service.recognize_image_after_read(image)
            print(proba)
            print(resultName + "-" + calculateName)
        except:
            continue
        if resultName == "unknown":
            totalNotInDb += 1
            if resultName == calculateName:
                trueNegative += 1
                result.append((resultName, calculateName, proba, "TN"))
            else:
                result.append((resultName, calculateName, proba, "FP"))
        else:
            totalInDb += 1
            if resultName == calculateName:
                truePositive += 1
                result.append((resultName, calculateName, proba, "TP"))
            else:
                if calculateName != "unknown":
                    falseNegative1 += 1
                    result.append((resultName, calculateName, proba, "FN1"))
                else:
                    falseNegative2 += 1
                    result.append((resultName, calculateName, proba, "FN2"))
    falseNegative = totalInDb - truePositive
    falsePositive = totalNotInDb - trueNegative
    print("Total true positive = {}", truePositive)
    print("Total false negative = {}", totalInDb - truePositive)
    print("Total false negative1 = {}", falseNegative1)
    print("Total false negative2 = {}", falseNegative2)
    print("Total true negative = {}", trueNegative)
    print("Total false positive = {}", totalNotInDb - trueNegative)

    print("Sensitivity [TP / (TP + FN)] = {}", truePositive / totalInDb)
    print("Precision [TP / (TP + FP)] = {}", truePositive / (truePositive + falsePositive))
    print("Accuracy [(TP + TN) / (TP + FP + TN + FN)] = {}", (truePositive + trueNegative) / (totalInDb + totalNotInDb))

    f = open("output_dlib/result.pickle", "wb+")
    f.write(pickle.dumps(result))
    f.close()



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


def testThreshold():
    result = pickle.loads(open("output_dlib/result.pickle", "rb+").read())
    thresholdData = getThresholdData(0, 1, 0.1, result)
    drawChart(thresholdData)