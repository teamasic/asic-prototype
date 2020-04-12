# import the necessary packages
import argparse
import copy
from datetime import datetime
import time

import cv2
import imutils
from helper import my_service, recognition_api, my_face_detection
import multiprocessing as mp

if __name__ == "__main__":

    pool = mp.Pool()
    time.sleep(7)
    # construct the argument parser and parse the arguments
    ap = argparse.ArgumentParser()
    ap.add_argument("-i", "--image", default="images/class3.jpg",
                    help="path to input image")
    ap.add_argument("-u", "--upSample", default=1,
                    help="path to input image")
    args = vars(ap.parse_args())
    # Load arguments
    imagePath = args["image"]
    numberOfTimesToUpSample = args["upSample"]

    image = cv2.imread(imagePath)
    # image = imutils.resize(image, width=1920, height=1080)
    (h, w) = image.shape[:2]
    startTime = datetime.now()
    boxes = my_face_detection.face_locations(image)
    print(datetime.now() - startTime)
    startTime = datetime.now()
    print("start time" + startTime.strftime("%H:%M:%S"))
    resultFull = pool.starmap(my_service.get_label_after_detect_multiple, [(copy.deepcopy(image), [copy.deepcopy(box)]) for box in boxes])
    results = [result[0] for result in resultFull]

    # results = my_service.get_label_after_detect_multiple(image, boxes)
    print(datetime.now() - startTime)
    print(results)
    print(len(results))
    for result in results:
        (box, name, proba) = result
        (top, right, bottom, left) = box

        # draw the predicted face name on the image
        text = "{}: {:.2f}%".format(name, proba * 100)

        cv2.rectangle(image, (left, top), (right, bottom), (0, 0, 225), 1)
        y = top - 10 if top - 10 > 10 else top + 10
        cv2.putText(image, text, (left, y), cv2.FONT_HERSHEY_SIMPLEX,
                    0.45, (0, 0, 255), 2)
        # show the output image
    cv2.imshow("Image", image)
    cv2.waitKey(0)
