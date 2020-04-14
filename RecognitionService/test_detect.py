import copy
import os
from datetime import datetime

import cv2
from imutils import paths

from helper import my_face_detection


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

test_detect_image("images/class3.jpg")