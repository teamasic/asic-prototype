import imgaug.augmenters as iaa
# https://imgaug.readthedocs.io/en/latest/source/examples_basics.html
# https://imgaug.readthedocs.io/en/latest/source/overview_of_augmenters.html


def face_generate(images, multiplier=1):
    seq = iaa.Sequential([
        iaa.Fliplr(0.5),  # flip left-right 50% of the time

        # Small gaussian blur with random sigma between 0 and 0.5.
        # But we only blur about 30% of all images.
        iaa.Sometimes(
            0.4,
            iaa.GaussianBlur(sigma=(0, 0.2))
        ),

        # Apply affine transformations to each image.
        # Scale/zoom them, translate/move them, rotate them and shear them.
        iaa.Affine(
            scale={"x": (0.95, 1.05), "y": (0.95, 1.05)},
            rotate=(-15,15),
            shear=(-5, 5),
            mode="edge"
        ),

        iaa.OneOf([
            iaa.MultiplySaturation((1.0, 2.5)),
            iaa.Grayscale(alpha=(0.0, 1.0)),
            iaa.MultiplyBrightness((0.8, 1.4))
        ])
    ], random_order=True)  # apply augmenters in random order
    return seq(images=images * multiplier)