import argparse

from helper import my_service

ap = argparse.ArgumentParser()
ap.add_argument("-i", "--dataset", default="dataset",
                help="path to input directory of faces + images")
args = vars(ap.parse_args())

my_service.generate_embeddings(args["dataset"])
