import os

from imutils import paths

from helper import my_service

fullDatasetDir = "dataset"
testDir = "images"
trainingDir = "training"
imagePathsToTest = list(paths.list_images(testDir))
total = 0
right = 0
rightList =[]
for imagePath in imagePathsToTest:
    print(imagePath)
    try:
        resultName = imagePath.split(os.path.sep)[-2]
        calculateName = my_service.recognize_image(imagePath)
        print(resultName + "-" + calculateName)
    except:
        continue
    total+=1
    if (resultName == calculateName):
        rightList.append(resultName + "-" + imagePath )
        right+=1
print((right/total)*100, "%")
for item in rightList:
    print(item)