import pandas as pd
import streamlit as st

from api_client import fetch_json, get_base_url

st.title("Stop Search")
base_url = get_base_url()

search = st.text_input("Suche (Name oder ID)", value="")
col1, col2 = st.columns(2)
limit = col1.number_input("Limit", min_value=1, max_value=500, value=50)
offset = col2.number_input("Offset", min_value=0, value=0)

if st.button("Laden"):
    data = fetch_json(f"{base_url}/api/stops", {"search": search, "limit": limit, "offset": offset})
    items = data.get("items", [])
    if not items:
        st.info("Keine Treffer")
    else:
        df = pd.DataFrame(items)
        st.dataframe(df, use_container_width=True)

        selected = st.selectbox("Stop auswählen", df["stopId"])
        if st.button("Details anzeigen"):
            st.session_state.selected_stop_id = selected

if st.session_state.get("selected_stop_id"):
    st.subheader(f"Details: {st.session_state.selected_stop_id}")
    details = fetch_json(f"{base_url}/api/stops/{st.session_state.selected_stop_id}")
    st.json(details)