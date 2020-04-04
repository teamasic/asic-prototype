import os
import uuid
from multiprocessing.context import Process
from threading import Thread

import cv2

from config import my_constant


def saveImageFunction(image, box):
    (top, right, bottom, left) = box
    (h, w) = image.shape[:2]
    if top - 50 < 0:
        top = 0
    else:
        top = top - 50

    if bottom + 50 > h:
        bottom = h
    else:
        bottom = bottom + 50

    if left - 50 < 0:
        left = 0
    else:
        left -= 50

    if right + 50 > w:
        right = w
    else:
        right = right + 50

    crop_image = image[top:bottom, left:right]

    unique_filename = "Unknown_{}.jpg".format(str(uuid.uuid4()))
    fullPath = os.path.join(my_constant.unknownDir, unique_filename)

    cv2.imwrite(fullPath, crop_image)
    return unique_filename


def remove_all_files(folder):
    for filename in os.listdir(folder):
        file_path = os.path.join(folder, filename)
        try:
            if os.path.isfile(file_path) or os.path.islink(file_path):
                os.unlink(file_path)
        except Exception as e:
            pass
