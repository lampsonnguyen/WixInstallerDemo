python hsm_manager.py <slot> <user_pin>



import pycryptoki
from pycryptoki.default_templates import get_default_key_template
from pycryptoki.cryptoki import CKA_LABEL, CKA_OBJECT_ID, CKR_OK
from pycryptoki.defines import CKO_SECRET_KEY, CKO_PRIVATE_KEY, CKO_PUBLIC_KEY, CKO_CERTIFICATE
from pycryptoki.session_management import c_open_session, c_close_session, c_logout, c_login
from pycryptoki.key_generator import c_destroy_object
import argparse

def show_all_objects(session):
    # Prepare search template
    search_template = {}

    # Find all objects
    found_objects = pycryptoki.misc.find_objects(session, template=search_template, max_count=100)
    if not found_objects:
        print("No objects found.")
        return

    for obj in found_objects:
        obj_label = pycryptoki.misc.get_attribute_value(session, obj, [CKA_LABEL])[0].to_dict().get(CKA_LABEL, b"").decode()
        obj_id = pycryptoki.misc.get_attribute_value(session, obj, [CKA_OBJECT_ID])[0].to_dict().get(CKA_OBJECT_ID, None)
        print(f"Object Label: {obj_label}, Object ID: {obj_id}")

def delete_object(session, object_id):
    # Prepare search template
    search_template = {
        CKA_OBJECT_ID: object_id
    }

    # Find the object
    found_objects = pycryptoki.misc.find_objects(session, template=search_template, max_count=1)
    if not found_objects:
        print("Object not found.")
        return

    object_handle = found_objects[0]
    
    # Destroy the object
    result = c_destroy_object(session, object_handle)
    if result == CKR_OK:
        print("Object deleted successfully.")
    else:
        print(f"Failed to delete object. Return code: {result}")

def main():
    parser = argparse.ArgumentParser(description="HSM Object Management Script.")
    parser.add_argument("slot", type=int, help="Slot number of the HSM")
    parser.add_argument("user_pin", type=str, help="User PIN for the HSM")

    args = parser.parse_args()

    # Initialize PyCryptoki
    pycryptoki.lib_initialize()

    # Open a session
    session = c_open_session(args.slot)
    
    try:
        # Log in as a user
        c_login(session, args.user_pin)
        
        while True:
            print("\nOptions:")
            print("1: Show all objects' names and IDs")
            print("2: Delete object using the object ID")
            print("3: End")
            choice = input("Choose an option (1, 2, or 3): ")

            if choice == '1':
                show_all_objects(session)
            elif choice == '2':
                object_id = int(input("Enter the object ID to delete: "))
                delete_object(session, object_id)
            elif choice == '3':
                break
            else:
                print("Invalid option. Please choose 1, 2, or 3.")
    finally:
        # Logout and close the session
        c_logout(session)
        c_close_session(session)
        
        # Finalize PyCryptoki
        pycryptoki.lib_finalize()

if __name__ == "__main__":
    main()
