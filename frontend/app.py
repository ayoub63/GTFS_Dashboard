import streamlit as st

st.set_page_config(page_title="GTFS Stop Explorer", layout="wide")

if "selected_stop_id" not in st.session_state:
    st.session_state.selected_stop_id = None
if "selected_station_id" not in st.session_state:
    st.session_state.selected_station_id = None

st.title("GTFS Stop Explorer & Analytics")
st.write("Nutze die Seiten in der Sidebar: Stop Search, Map Explorer, Station Hierarchy und Analytics.")