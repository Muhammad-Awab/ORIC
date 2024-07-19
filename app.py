import os
from PIL import Image
import streamlit as st
from io import BytesIO

def convert_and_save(uploaded_file):
    if uploaded_file is not None:
        # Open the uploaded file as an image
        img = Image.open(uploaded_file)
        rgb_img = img.convert('RGB')
        # Save the image in JPEG format to a BytesIO object
        img_byte_arr = BytesIO()
        rgb_img.save(img_byte_arr, format='JPEG')
        img_byte_arr.seek(0)
        return img_byte_arr
    return None

def main():
    st.title("PNG to JPEG Converter")

    # File uploader for multiple PNG files
    uploaded_files = st.file_uploader("Choose PNG files", type="png", accept_multiple_files=True)

    if st.button("Convert to JPEG"):
        if uploaded_files:
            converted_files = []
            for uploaded_file in uploaded_files:
                img_byte_arr = convert_and_save(uploaded_file)
                if img_byte_arr:
                    file_name = os.path.splitext(uploaded_file.name)[0] + '.jpeg'
                    converted_files.append((file_name, img_byte_arr))

            for file_name, img_byte_arr in converted_files:
                st.download_button(
                    label=f"Download {file_name}",
                    data=img_byte_arr,
                    file_name=file_name,
                    mime='image/jpeg'
                )
                st.success(f"Converted and ready for download: {file_name}")
        else:
            st.error("No files uploaded for conversion")

if __name__ == "__main__":
    main()
