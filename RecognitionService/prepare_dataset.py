import cv2;
import os;

from helper import my_face_detection



def detect_and_crop_faces(filePath):
    crops = [];
    image = cv2.imread(filePath);
    boxes = my_face_detection.face_locations(image);
    for (i, (top, right, bottom, left)) in enumerate(boxes):
        crop = image[top:bottom, left: right];
        # cv2.cvtColor(crop, cv2.COLOR_BGR2GRAY);
        crops.append(crop);
    return crops;

def detect_and_crop_face(filePath):
    image = cv2.imread(filePath);
    boxes = my_face_detection.face_locations(image);
    top, right, bottom, left = boxes[0];
    crop = image[top:bottom, left: right];
    return crop;

def save_image(fileName, image):
    cv2.imwrite(fileName, image);
    return "";



rawFolder = "J:\\DataDownloader\\DownloadImage\\bin\\Release\\raw";
cropFolder = "J:\\DataDownloader\\DownloadImage\\bin\\Release\\crop"
listDir = os.listdir(rawFolder);

for dirname in listDir:
    dirpath = os.path.join(rawFolder, dirname);
    print(dirname)
    saveToDir = os.path.join(cropFolder, dirname);
    if(os.path.exists(saveToDir) == False):
        os.mkdir(saveToDir);
    for filename in os.listdir(dirpath):
        try:
            # process multi faces
            filepath = os.path.join(dirpath, filename)
            crops = detect_and_crop_faces(filepath)
            for (i, crop) in enumerate(crops):
                filename = filename.split('.jpg')[0] + "_"+str(i)+".jpg";
                saveTo = os.path.join(cropFolder, dirname, filename);
                save_image(saveTo, crop);
                print(filename)

        # process one face in one image
        #     filepath = os.path.join(dirpath, filename)
        #     if(os.path.exists(filepath) == False):
        #         crop = detect_and_crop_face(filepath)
        #         saveTo = os.path.join(cropFolder, dirname, filename);
        #         save_image(saveTo, crop);
        #         print(filename)
        except:
            print("ignore error");
