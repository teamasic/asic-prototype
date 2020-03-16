import os

from imutils import paths

from helper import my_service

fullDatasetDir = "dataset"
testDir = "images"
trainingDir = "training"
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
        totalNotInDb+=1
        if resultName == calculateName:
            trueNegative+=1
    else:
        totalInDb+=1
        if resultName == calculateName:
            truePositive+=1
falseNegative = totalInDb - truePositive
falsePositive = totalNotInDb - trueNegative
print("Total true positive = {}", truePositive)
print("Total false negative = {}", totalInDb - truePositive)
print("Total true negative = {}", trueNegative)
print("Total false positive = {}", totalNotInDb - trueNegative)

print("Sensitivity [TP / (TP + FN)] = {}", truePositive/totalInDb)
print("Precision [TP / (TP + FP)] = {}", truePositive/(truePositive + falsePositive))
print("Accuracy [(TP + TN) / (TP + FP + TN + FN)] = {}", (truePositive + trueNegative)/(totalInDb + totalNotInDb))
