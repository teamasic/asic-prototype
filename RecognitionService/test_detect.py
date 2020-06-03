import copy
import os
import shutil
from datetime import datetime
from pathlib import Path

import cv2
import imutils
from imutils import paths

from helper import my_face_detection, my_utils


def test_detect_time(dir):
    sum = 0
    image_paths = paths.list_images(dir)
    count = 0
    for image_path in list(image_paths):
        count+=1
        start_time = datetime.now()
        image = cv2.imread(image_path)
        (h, w) = image.shape[:2]
        my_face_detection.face_locations(image)
        statinfo = os.stat( image_path)
        calculation_time = datetime.now() - start_time
        sum += w*h/calculation_time.total_seconds()
        print(f"{calculation_time} - {w} x {h} - {w*h} - {w*h/calculation_time.total_seconds()}- {statinfo.st_size/1024} - {image_path}")
    print(f"Avarage each second, can detect {sum/count}")
# test_detect_time("images_detect")

def test_detect_image(imagePath):
    startTime = datetime.now()
    image = cv2.imread(imagePath)
    boxes = my_face_detection.face_locations(image)
    print(len(boxes))
    print(datetime.now() - startTime)

def test_detect_full(dir, removed=False):
    imagePaths = list(paths.list_images(dir))
    print(len(imagePaths))
    listFailed = []
    failedDetectDir = "temp/failed_detect"
    for (i, imagePath) in enumerate(imagePaths):
        print("Try detect image {}/{}".format(i + 1, len(imagePaths)))
        try:
            image = cv2.imread(imagePath)
            # image = imutils.resize(image, width=400)
            image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        except:
            print(imagePath + "not ok")
            continue
        boxes = my_face_detection.face_locations(image)
        if len(boxes) != 1:
            print("FALSE - {} - Length: {}".format(imagePath, len(boxes)))
            listFailed.append(imagePath)

            Path(failedDetectDir).mkdir(parents=True, exist_ok=True)
            shutil.copy(imagePath, failedDetectDir)

            if (removed is True):
                os.remove(imagePath)
    print("Failed {}/{}".format(len(listFailed), len(imagePaths)))


def test_detect_image2(path, removed=False):
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
def detectAndCrop(origin_dir, target_dir):
    imagePaths = list(paths.list_images(origin_dir))
    for (i, imagePath) in enumerate(imagePaths):
        # extract the person name from the image path
        print("[INFO] processing image {}/{}".format(i + 1,
                                                     len(imagePaths)))

        name = imagePath.split(cv2.os.path.sep)[-2]

        # load the image
        image = cv2.imread(imagePath)
        image = imutils.resize(image, width=400)
        imageRaw = copy.deepcopy(image)
        image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        boxes = my_face_detection.face_locations(image)
        if len(boxes) > 1:
            print(imagePath, "> 1")
            print(len(boxes))
            continue
        if len(boxes) == 0:
            print(imagePath, "= 0")
            print(len(boxes))
            continue
        pathimage = os.path.join(target_dir, name)
        Path(pathimage).mkdir(parents=True, exist_ok=True)
        my_utils.saveImageFunction(imageRaw, boxes[0], pathimage, 40)
detectAndCrop(r"D:\dataOK", r"D:\dataOkCrop")
test_detect_full(r"D:\dataOkCrop", removed=True)