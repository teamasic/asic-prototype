# import the necessary packages
import argparse
import cv2
import imutils
from helper import my_service, recognition_api

# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-i", "--image", default="images/unknown/1.jpg",
                help="path to input image")
ap.add_argument("-u", "--upSample", default=3,
                help="path to input image")
args = vars(ap.parse_args())
# Load arguments
imagePath = args["image"]
numberOfTimesToUpSample = args["upSample"]

image = cv2.imread(imagePath)
(h, w) = image.shape[:2]
results = my_service.recognize_image_after_read_multiple(image, numberOfTimesToUpSample=numberOfTimesToUpSample)
for result in results:
    (box, name, proba) = result
    (top, right, bottom, left) = box
    # Show and call API
    # if name != "unknown":
    #     recognition_api.recognize_face_new_thread(name)

    # draw the predicted face name on the image
    text = "{}: {:.2f}%".format(name, proba * 100)

    cv2.rectangle(image, (left, top), (right, bottom), (0, 0, 225), 1)
    y = top - 10 if top - 10 > 10 else top + 10
    cv2.putText(image, text, (left, y), cv2.FONT_HERSHEY_SIMPLEX,
                0.45, (0, 0, 255), 2)
    # show the output image
cv2.imshow("Image", image)
cv2.waitKey(0)
