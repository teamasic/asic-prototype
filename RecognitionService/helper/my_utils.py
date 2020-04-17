import os
import uuid
from multiprocessing.context import Process
from threading import Thread

import cv2
import imutils

from config import my_constant


def saveImageFunction(image, box, dir=my_constant.unknownDir, num=50):
    (top, right, bottom, left) = box
    (h, w) = image.shape[:2]
    if top - num < 0:
        top = 0
    else:
        top = top - num

    if bottom + num > h:
        bottom = h
    else:
        bottom = bottom + num

    if left - num < 0:
        left = 0
    else:
        left -= num

    if right + num > w:
        right = w
    else:
        right = right + num

    crop_image = image[top:bottom, left:right]
    crop_image = imutils.resize(crop_image, width=250)

    unique_filename = "Unknown_{}.jpg".format(str(uuid.uuid4()))
    fullPath = os.path.join(dir, unique_filename)

    cv2.imwrite(fullPath, crop_image)
    return unique_filename


def remove_all_files(folder):
    if os.path.exists(folder):
        for filename in os.listdir(folder):
            file_path = os.path.join(folder, filename)
            try:
                if os.path.isfile(file_path) or os.path.islink(file_path):
                    os.unlink(file_path)
            except Exception as e:
                pass
    else:
        raise Exception("Folder {} does not exist".format(folder))
