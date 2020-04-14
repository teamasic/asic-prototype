class BooleanWrapper:
    def __init__(self, value):
        self.value = value

    def change(self, value):
        self.value = value

    def isTrue(self):
        return self.value
