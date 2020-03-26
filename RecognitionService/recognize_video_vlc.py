# import the necessary packages
from config import my_constant
import os
os.add_dll_directory(my_constant.vlcPath)
import time
import vlc
import argparse
import cv2
import imutils
from helper import recognition_api, my_service


# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-p", "--rtsp", default="rtsp://192.168.1.6:8554/unicast",
                help="path to rtsp string")
args = vars(ap.parse_args())

# Load arguments
rtspString = args["rtsp"]

# Play the video stream
player = vlc.MediaPlayer(rtspString)
player.play()

# wait 1 second to connect
time.sleep(1)

while True:
    time.sleep(1)
    snapshotResult = player.video_take_snapshot(0, my_constant.snapshotPath, 0, 0)
    if snapshotResult == 0:
        image = cv2.imread(my_constant.snapshotPath)
        image = imutils.resize(image, width=600)
        (h, w) = image.shape[:2]

        result = my_service.recognize_image_after_read(image)
        if result is not None:
            (box, name, proba) = result
            (top, right, bottom, left) = box
            # Show and call API
            recognition_api.recognize_face_new_thread(name)

            # draw the predicted face name on the image
            text = "{}: {:.2f}%".format(name, proba * 100)

            cv2.rectangle(image, (left, top), (right, bottom), (0, 0, 225), 2)
            y = top - 10 if top - 10 > 10 else top + 10
            cv2.putText(image, text, (left, y), cv2.FONT_HERSHEY_SIMPLEX,
                        0.45, (0, 0, 255), 2)
            # show the output image
        cv2.imshow("Image", image)
        cv2.waitKey(1)
    else:
        raise Exception("Cannot read video stream")
