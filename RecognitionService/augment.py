import argparse
import os
import pickle
import numpy as np
import imgaug
import cv2
import face_recognition
import imutils
import shutil

# import the necessary packages
from imutils import paths

# construct the argument parser and parse the arguments
from helper import my_face_detection, my_face_recognition, my_face_generator

ap = argparse.ArgumentParser()
ap.add_argument("-i", "--dataset", default="dataset",
                help="path to input directory of faces + images")
ap.add_argument("-o", "--output", default="augmented",
                help="path to output directory of augmented images")

args = vars(ap.parse_args())

# grab the paths to the input images in our dataset
print("[INFO] quantifying faces...")
imagePaths = list(paths.list_images(args["dataset"]))
augmented_path = args["output"]

# initialize our lists of extracted facial embeddings and
# corresponding people names
knownEmbeddings = []
knownNames = []

# initialize the total number of faces processed
total = 0

unknown_batch = []
original_batch = []
name_batch = []
count = 4  # generate 4 fake images for 1 raw image

# loop over the image paths
for (i, imagePath) in enumerate(imagePaths):
    # extract the person name from the image path
    print("[INFO] processing image {}/{}".format(i + 1,
                                                 len(imagePaths)))

    name = imagePath.split(os.path.sep)[-2]

    # load the image, resize it to have a width of 400 pixels (while
    # maintaining the aspect ratio)
    image = cv2.imread(imagePath)
    image = imutils.resize(image, width=400)
    if name == "unknown":
        unknown_batch.append(image)
        pass
    else:
        original_batch.append(image)
        name_batch.append(name)

augmented_batch = my_face_generator.face_generate(original_batch, count)
name_batch = name_batch * count

# add all augmented images into a dictionary
name_image_dict = dict()
for name, image in zip(name_batch, augmented_batch):
    if name in name_image_dict:
        name_image_dict[name].append(image)
    else:
        name_image_dict[name] = []
# add all unknown images into the dictionary
name_image_dict["unknown"] = unknown_batch

if os.path.exists(augmented_path):
    shutil.rmtree(augmented_path)
os.mkdir(augmented_path)
for name in name_image_dict.keys():
    os.mkdir(os.path.sep.join([augmented_path, name]))

# write each augmented image into their respective folder
for name, images in name_image_dict.items():
    for i, image in enumerate(images):
        full_file_name = os.path.sep.join([augmented_path, name, str(i + 1) + ".jpg"])
        cv2.imwrite(full_file_name, image)
