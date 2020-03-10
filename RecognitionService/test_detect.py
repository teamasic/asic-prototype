import cv2
from imutils import paths
from sklearn import datasets

from helper import my_face_detection


def testDetectFull():
    datasetDir = "dataset"
    imagePaths = paths.list_images(datasetDir)
    for imagePath in imagePaths:
        image = cv2.imread(imagePath)
        boxes = my_face_detection.face_locations(image)
        if len(boxes) != 1:
            print(imagePath)
            print(len(boxes))
def testDetectImamge(imagePath):
    image = cv2.imread(imagePath)
    boxes = my_face_detection.face_locations(image)
    print(boxes)
    if len(boxes) != 1:
        print(imagePath)
        print(len(boxes))

testDetectImamge("training/xuka/8.jpg")
testDetectFull()