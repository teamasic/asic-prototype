# import the necessary packages
from sklearn.preprocessing import LabelEncoder
from sklearn import svm
import argparse
import pickle
import numpy as np

# load the face embeddings
from config import my_constant

print("[INFO] loading face embeddings...")
data = pickle.loads(open(my_constant.embeddingsPath, "rb").read())

# encode the labels
print("[INFO] encoding labels...")
le = LabelEncoder()
labels = le.fit_transform(data["names"])


# train the model used to accept the 128-d embeddings of the face and
# then produce the actual face recognition
print("[INFO] training model...")
recognizer = svm.SVC(C=1.0, kernel="linear", probability=True)
recognizer.fit(data["embeddings"], labels)

# write the actual face recognition model to disk
f = open(my_constant.recognizerPath, "wb")
f.write(pickle.dumps(recognizer))
f.close()

# write the label encoder to disk
f = open(my_constant.lePath, "wb")
f.write(pickle.dumps(le))
f.close()