import timeit
from datetime import datetime

import cv2

from helper import my_face_detection
image = cv2.imread("../images/trong1.jpg")
start=datetime.now()
print(my_face_detection._face_locations_caffe(image))
print("Time caffe model: ",datetime.now() - start)
start=datetime.now()
print(my_face_detection._face_locations_hog(image))
print("Time hog: ",datetime.now() - start)
# start=datetime.now()
# print(my_face_detection._face_locations_cnn(image))
# print("Time cnn: ", datetime.now() - start)
