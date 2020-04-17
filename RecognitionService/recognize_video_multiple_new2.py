import argparse
import copy
import multiprocessing
import threading
import time
import tkinter as tk
from datetime import datetime

import cv2
import imutils
from PIL import Image
from PIL import ImageTk
from imutils.video import FPS

from config import my_constant
from helper import my_service, my_face_detection, stream_video, recognition_api, my_utils, boolean_wrapper
from helper.my_utils import remove_all_files


def recognition_faces():
    isShowingVideo.change(False)
    startime = datetime.now()
    global currentImage
    # currentImage = cv2.imread("images/class3.jpg")
    currentImageRGB = cv2.cvtColor(currentImage, cv2.COLOR_BGR2RGB)
    boxes = my_face_detection.face_locations(currentImageRGB)
    resultFull = pool.starmap(my_service.get_label_after_detect_multiple,
                              [(currentImageRGB, [box]) for box in boxes])
    results = [result[0] for result in resultFull]
    unknowns = [x for x in results if x[1] == "unknown"]
    print(len(unknowns))
    print(len(results))
    print(results)
    if isForCheckingAttendance:
        lbTotalTime['text'] = "Checking attendance ..."
        codes = []
        unknowns = []
        for result in results:
            (box, name, proba) = result
            if name == "unknown":
                imageName = my_utils.saveImageFunction(currentImage, box)
                unknowns.append(imageName)
            else:
                codes.append(name)
        try:
            recognition_api.recognize_multiple_faces(codes, unknowns)
        except:
            cannotCallApi.change(True)
    print(f"Total Time: {datetime.now() - startime}")
    lbTotalTime['text'] = "Done! Total time: {} - {} faces".format(datetime.now() - startTimeRecognition, len(boxes))
    isShowingVideo.change(True)
    if videoStreamLock.locked():
        videoStreamLock.release()


def takeSnapshot():
    global startTimeRecognition
    startTimeRecognition = datetime.now()
    print("Image saved!")
    lbTotalTime["text"] = "Recognizing faces ..."
    threading.Thread(target=recognition_faces , daemon=True).start()


def show_frame():
    while True:
        if cannotCallApi.isTrue():
            window.quit()
        else:
            if isShowingVideo.isTrue():
                btnCapture["state"] = "normal"
                frame = vs.read()
                cv2image = imutils.resize(frame, width=my_constant.resizeWidthShow)
                global currentImage
                currentImage = copy.deepcopy(cv2image)

                cv2image = cv2.cvtColor(cv2image, cv2.COLOR_BGR2RGB)
                img = Image.fromarray(cv2image)
                imgtk = ImageTk.PhotoImage(image=img)
                fps.update()
                lbImage.configure(image=imgtk)
                lbImage.imgtk = imgtk
            else:
                btnCapture["state"] = "disabled"
                videoStreamLock.acquire()


if __name__ == "__main__":
    # get arguments
    ap = argparse.ArgumentParser()
    ap.add_argument("-p", "--rtsp", default="rtsp://192.168.1.4:8554/unicast",
                    help="path to rtsp string")
    ap.add_argument("-a", "--attendance", default=False,
                    help="Open video stream for checking attendance or not")
    ap.add_argument("-b", "--box", default=False,
                    help="Show box in video")
    args = vars(ap.parse_args())

    # Load arguments
    rtspString = args["rtsp"]
    isForCheckingAttendance = (str(args["attendance"]) == "True")
    isShowBox = (str(args["box"]) == "True")

    # Setup
    print("[INFO] Warming up...")
    fps = FPS()
    fps.start()
    pool = multiprocessing.Pool()
    currentImage = None
    isShowingVideo = boolean_wrapper.BooleanWrapper(True)
    cannotCallApi = boolean_wrapper.BooleanWrapper(False)
    startTimeRecognition = None
    videoStreamLock = threading.Lock()

    # Setup gui
    window = tk.Tk()
    window.wm_title("ASIC checking attendance")
    window.resizable(False, False)

    btnCapture = tk.Button(window, text="Capture", bg="#000099", fg="#ffffff",
                           command=lambda: takeSnapshot())
    btnCapture.grid(row=2, column=0, padx=10, pady=5, sticky='EWNS')

    btnExit = tk.Button(window, text="Exit", command=window.quit)
    btnExit.grid(row=2, column=1, padx=10, pady=5, sticky='EWNS')

    lbTotalTime = tk.Label(window, text="Ready for checking attendance ...")
    lbTotalTime.grid(row=1, column=0, columnspan=2, padx=10, pady=5)

    lbImage = tk.Label(window)
    lbImage.grid(row=0, column=0, columnspan=2, padx=10, pady=5)

    # Warm up camera
    isOpenStreamOk = False
    countTryOpenStream = 0
    timesTryConnect = 3
    vs = None
    try:
        # Try connect camera 3 times
        while countTryOpenStream < timesTryConnect and isOpenStreamOk is False:
            countTryOpenStream += 1
            if countTryOpenStream == 1:
                httpString = "http://localhost:{}".format(my_constant.portHttpStream)
            else:
                httpString = my_service.transfer_rtsp_to_http(rtspString)
            time.sleep(2.0)
            vs = stream_video.CustomVideoStream(src=httpString)
            if vs.stream.isOpened():
                isOpenStreamOk = True

        # If cannot connect -> raise exception
        if isOpenStreamOk is False:
            raise Exception("Cannot read video stream")

        # If connect OK -> continue work
        vs.start()
        threading.Thread(target=show_frame, daemon=True).start()
        window.mainloop()

        # Check if be able to call api
        if cannotCallApi.isTrue():
            raise Exception("Cannot checking attendance")

        fps.stop()
        print("FPS: ", fps.fps())

    finally:
        # Clean
        cv2.destroyAllWindows()
        vs.stop()
        pool.close()
        remove_all_files(my_constant.unknownDir)
