import os
import uuid
from multiprocessing.context import Process
from os import path
from pathlib import Path
from threading import Thread

import cv2
import imutils

from config import my_constant


def saveImageFunction(image, box, dir, padding=20, imageName = None, isOverwrite=False):
    Path(dir).mkdir(exist_ok=True)
    (top, right, bottom, left) = box
    (h, w) = image.shape[:2]
    cv2.rectangle(image, (left, top), (right, bottom), (0, 0, 225), 1)
    newTop = 0 if top - padding < 0 else top - padding
    newBottom = h if bottom + padding > h else bottom + padding
    newLeft = 0 if left - padding < 0 else left - padding
    newRight = w if right + padding > w else right + padding

    crop_image = image[newTop:newBottom, newLeft:newRight]
    crop_image = cv2.resize(crop_image, (150, 150), interpolation=cv2.INTER_AREA)
    if imageName is None:
        imageName = "{}.jpg".format(str(uuid.uuid4()))
    fullPath = os.path.join(dir, imageName)
    successSave = False
    if (path.exists(fullPath) is False) or (path.exists(fullPath) is True and isOverwrite):
        successSave = cv2.imwrite(fullPath, crop_image)
    return imageName, successSave


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