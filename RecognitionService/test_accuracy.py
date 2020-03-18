from helper import my_service, my_test
from helper.my_test import split_train_test

split_train_test(numOfTrain=3, numOfTest=4)
my_service.augment_images(datasetDir="training", augmentedDir="augmented")
my_service.generate_embeddings("augmented")
my_service.generate_train_model()
my_test.test_result()
