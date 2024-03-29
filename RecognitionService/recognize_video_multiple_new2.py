import argparse
import copy
import multiprocessing
import os
import threading
import time
import tkinter as tk
from datetime import datetime
from os import path

import cv2
import imutils
from PIL import Image
from PIL import ImageTk
from imutils.video import FPS

from config import my_constant
from helper import my_service, my_face_detection, stream_video, recognition_api, my_utils, boolean_wrapper
from helper.my_utils import remove_all_files


def recognition_faces():
    startime = datetime.now()
    global currentImage, sessionId
    # currentImage = cv2.imread(r"C:\Users\thanh\Desktop\123.jpg")
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
        codes = set()
        unknowns = []
        for result in results:
            (box, name, proba, vec) = result
            if name == "unknown":
                pathUnknown = path.join(my_constant.unknownDir, str(sessionId))
                imageName, successSave = my_utils.saveImageFunction(copy.deepcopy(currentImage), box, dir=pathUnknown)
                print(imageName, successSave)
                if successSave:
                    unknowns.append(imageName)
            else:
                pathPeople = path.join(my_constant.peopleDir, str(sessionId))
                imageName = name + ".jpg"
                imageName, successSave = my_utils.saveImageFunction(copy.deepcopy(currentImage), box, dir=pathPeople, imageName=imageName)
                if successSave:
                    codes.add(name)
        try:
            recognition_api.recognize_multiple_faces(list(codes), unknowns)
        except:
            cannotCallApi.change(True)
    print(f"Total Time: {datetime.now() - startime}")
    lbTotalTime['text'] = "Done! Total time: {} - {} faces".format(datetime.now() - startTimeRecognition, len(boxes))
    isShowingVideo.change(True)
    if videoStreamLock.locked():
        videoStreamLock.release()


def takeSnapshot():
    isShowingVideo.change(False)
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
                cv2image = imutils.resize(frame, width=my_constant.resizeWidthRecognize)
                global currentImage
                currentImage = copy.deepcopy(cv2image)

                cv2image = imutils.resize(frame, width=my_constant.resizeWidthShow)
                cv2image = cv2.cvtColor(cv2image, cv2.COLOR_BGR2RGB)
                img = Image.fromarray(cv2image)
                imgtk = ImageTk.PhotoImage(image=img)
                fps.update()
                lbImage.configure(image=imgtk)
                lbImage.imgtk = imgtk
            else:
                btnCapture["state"] = "disabled"
                videoStreamLock.acquire()


def changePositionWindownFromRight(width, height, fromRight, fromTop):
    screen_width = window.winfo_screenwidth()
    screen_height = window.winfo_screenheight()
    x = screen_width - fromRight - width
    y = fromTop
    window.geometry('%dx%d+%d+%d' % (width, height, x, y))


if __name__ == "__main__":
    # get arguments
    ap = argparse.ArgumentParser()
    ap.add_argument("-p", "--rtsp", default="rtsp://192.168.1.29:8554/unicast",
                    help="path to rtsp string")
    ap.add_argument("-a", "--attendance", default=False,
                    help="Open video stream for checking attendance or not")
    ap.add_argument("-b", "--box", default=False,
                    help="Show box in video")
    ap.add_argument("-s", "--sessionId", default=1,
                    help="Session Id")
    args = vars(ap.parse_args())

    # Load arguments
    rtspString = args["rtsp"]
    isForCheckingAttendance = (str(args["attendance"]) == "True")
    isShowBox = (str(args["box"]) == "True")
    sessionId = int(args["sessionId"])

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

    # change position window
    changePositionWindownFromRight(620, 425, 50, 50)

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
                httpString = my_service.transfer_rtsp_to_http(rtspString)
            else:
                httpString = my_service.transfer_rtsp_to_http(rtspString)
            vs = stream_video.CustomVideoStream(src=httpString)
            if vs.stream.isOpened():
                isOpenStreamOk = True
            else:
                vs.stop()
                time.sleep(2)

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
        os.system("terminate_vlc.bat")
