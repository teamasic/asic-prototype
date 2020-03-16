import os
import shutil

from imutils import paths

datasetDir = "E:\\capstone\\lfwpeople\\lfw-funneled\\lfw-funneled\\lfw_funneled"
unknownDir = "E:\\capstone\\total_dataset\\unknown"

listImages = []
totalImage = 500
for (root, subDirs, files) in os.walk(datasetDir):
    for subDir in subDirs:
        fullSubDir = os.path.join(datasetDir, subDir)
        image = paths.list_images(fullSubDir).__next__()
        if (len(listImages) < 500):
            listImages.append(image)
            shutil.copy(image, unknownDir)
        else:
            break
    break
print(len(listImages))