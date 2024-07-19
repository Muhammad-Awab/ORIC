import os
from PIL import Image
import streamlit as st
from io import BytesIO
import shutil
import tempfile

def convert_and_save(uploaded_file, save_path):
    if uploaded_file is not None:
        # Open the uploaded file as an image
        img = Image.open(uploaded_file)
        rgb_img = img.convert('RGB')
        # Construct the new file name and path
        file_name = os.path.splitext(uploaded_file.name)[0] + '.jpeg'
        save_file_path = os.path.join(save_path, file_name)
        # Save the image in JPEG format
        rgb_img.save(save_file_path, 'JPEG')
        return save_file_path
    return None

def main():
    st.title("PNG to JPEG Converter")

    # File uploader for multiple PNG files
    uploaded_files = st.file_uploader("Choose PNG files", type="png", accept_multiple_files=True)

    # Input for folder path (you can use a text input or a different method to select the folder)
    save_folder = st.text_input("Enter folder path to save JPEG files")

    if st.button("Convert and Save"):
        if uploaded_files and save_folder:
            # Ensure the folder exists
            if not os.path.exists(save_folder):
                os.makedirs(save_folder)

            for uploaded_file in uploaded_files:
                save_path = convert_and_save(uploaded_file, save_folder)
                if save_path:
                    st.success(f"Converted and saved to {save_path}")
                else:
                    st.error("Failed to convert the file")
        else:
            st.error("Please upload files and specify a folder path")

if __name__ == "__main__":
    main()
