import pickle
import shutil

import cv2
import numpy as np
from imutils import paths
import sklearn
from sklearn import svm, metrics
from sklearn.preprocessing import LabelEncoder
from sklearn.model_selection import KFold, cross_val_score, GridSearchCV
from config import my_constant
from matplotlib import pyplot as plt
import os


def generate_train_model():
    embedding = pickle.loads(open(my_constant.embeddingsPath, "rb").read())
    X = embedding["embeddings"]
    le = LabelEncoder();
    Y = le.fit_transform(embedding["names"])
    # Y = LabelEncoder().fit_transform(embedding["names"])
    print(len(X))
    print(len(Y))

    x_train, x_test, y_train, y_test = \
        sklearn.model_selection.train_test_split(X, Y, test_size=0.2)

    # xx_train, xx_validation, yy_train, yy_validation = \
    #     sklearn.model_selection.train_test_split(x_train, y_train, test_size=0.2)

    # x_axis = range(1, 10)
    # y_axis = []
    # yy_axis = []

    # param_grid = [
    #     {'C': [1, 10, 100, 1000],
    #      'kernel': ['linear']},
    #     {'C': [1, 10, 100, 1000],
    #      'gamma': [0.001, 0.0001],
    #      'kernel': ['rbf']}
    # ]

    clf = svm.SVC(C=100, gamma=1, kernel="rbf", probability=True)
    # clf = GridSearchCV(svm.SVC(C=1, probability=True), param_grid, cv=4)
    # clf.fit(X, Y)

    # for i in x_axis:
    clf.fit(x_train, y_train)
    print("predict")
    y_pred = clf.predict(x_test)
    acc = metrics.accuracy_score(y_test, y_pred)

    print("test: " + str(acc))

    # print(clf.best_params_)
    # print(clf.best_score_)
    # print(clf.best_index_)
    # print(clf.best_estimator_)

    # y_axis.append(accTest * 100)
    # yy_axis.append(accVal * 100)

    # plt.plot(x_axis, yy_axis)
    # plt.plot(x_axis, y_axis)
    # plt.legend(["val acc", "test acc"])
    # plt.show();

    # write the actual face recognition model to disk
    f = open(my_constant.recognizerPath, "wb")
    f.write(pickle.dumps(clf))
    f.close()

    # write the label encoder to disk
    f = open(my_constant.lePath, "wb")
    f.write(pickle.dumps(le))
    f.close()


def train_tune():
    embedding = pickle.loads(open(my_constant.embeddingsPath, "rb").read())
    X = embedding["embeddings"]
    le = LabelEncoder()
    Y = le.fit_transform(embedding["names"])
    print(len(X))
    C_range = 10. ** np.arange(-2, 5)
    gamma_range = 10. ** np.arange(-6, 3)
    param_grid = [{'C': C_range, 'gamma': gamma_range, 'kernel': ['rbf']}]
    clf = GridSearchCV(svm.SVC(), param_grid, cv=3)
    clf.fit(X, Y)
    print(clf.best_params_)
    print(clf.best_score_)
    print(clf.best_estimator_)

    f = open(my_constant.recognizerPath, "wb")
    f.write(pickle.dumps(clf))
    f.close()

    f = open(my_constant.lePath, "wb")
    f.write(pickle.dumps(le))
    f.close()



generate_train_model()
# train_tune()


# check features range [0-1]
# src = "J:\\asic-prototype\\RecognitionService\\output_dlib\\embeddings.pickle"
# embeddings = pickle.loads(open(src, "rb").read())
# vectors = embeddings["embeddings"]
# for vec in vectors:
#     (print(vec) for e in vec if (e >= 1 or e <= -1))



# append embedding files
# src = "J:\\asic-prototype\\RecognitionService\\output_dlib\\309-test-embeddings.pickle"
# dest ="j:\\embeddings.pickle"
# total = "J:\\asic-prototype\\RecognitionService\\output_dlib\\309-total-embeddings.pickle"
# embeddingS = pickle.loads(open(src, "rb").read())
# Xs = embeddingS["embeddings"]
# Ys = embeddingS["names"]
# embeddingD = pickle.loads(open(dest, "rb").read())
# Xd = embeddingD["embeddings"]
# Yd = embeddingD["names"]
# Xtotal = []
# Xtotal.extend(Xs)
# Xtotal.extend(Xd)
# Ytotal = []
# Ytotal.extend(Ys)
# Ytotal.extend(Yd)
# data = {"embeddings": Xtotal, "names": Ytotal}
# f = open(total, "wb")
# f.write(pickle.dumps(data))
# f.close()

# filter dataset on celeb
# path = "j:\\dataset\\VN-celeb"
# destPath ="J:\\asic-prototype\\RecognitionService\\dataset"
# [os.remove(os.path.join(destPath, dir)) for dir in os.listdir(destPath)]
# for dicName in os.listdir(path):
#     dic = os.path.join(path, dicName)
#     if(len(os.listdir(dic)) >= 20):
#         print(dic)
#         os.chmod(dic, os.st.S_IWRITE)
#         newDic = os.path.join(destPath, dicName)
#         os.mkdir(newDic)
#         shutil.copytree(dic, newDic, dirs_exist_ok=True)
