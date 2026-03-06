import pandas as pd
import pydeck as pdk
import streamlit as st

from api_client import fetch_json, get_base_url

st.title("Map Explorer")
base_url = get_base_url()

with st.form("bbox"):
    c1, c2 = st.columns(2)
    min_lat = c1.number_input("minLat", value=48.0, format="%.6f")
    max_lat = c2.number_input("maxLat", value=48.3, format="%.6f")
    c3, c4 = st.columns(2)
    min_lon = c3.number_input("minLon", value=11.3, format="%.6f")
    max_lon = c4.number_input("maxLon", value=11.8, format="%.6f")
    limit = st.number_input("limit", min_value=1, max_value=10000, value=1000)
    submit = st.form_submit_button("Stops im BBOX laden")

if submit:
    stops = fetch_json(
        f"{base_url}/api/stops/in-bbox",
        {"minLat": min_lat, "minLon": min_lon, "maxLat": max_lat, "maxLon": max_lon, "limit": limit},
    )
    if stops:
        df = pd.DataFrame(stops)
        st.dataframe(df[["stopId", "stopName", "stopLat", "stopLon", "locationType", "parentStation"]], use_container_width=True)

        layer = pdk.Layer(
            "ScatterplotLayer",
            data=df,
            get_position="[stopLon, stopLat]",
            get_fill_color="[200, 30, 0, 160]",
            get_radius=20,
            pickable=True,
        )
        view_state = pdk.ViewState(latitude=float(df.stopLat.mean()), longitude=float(df.stopLon.mean()), zoom=11)
        st.pydeck_chart(pdk.Deck(layers=[layer], initial_view_state=view_state, tooltip={"text": "{stopName} ({stopId})"}))

        choice = st.selectbox("Stop aus Kartenliste auswählen", df["stopId"])
        if st.button("Als selected stop setzen"):
            st.session_state.selected_stop_id = choice
    else:
        st.warning("Keine Stops im Bereich")

if st.session_state.get("selected_stop_id"):
    st.caption(f"Aktuell ausgewählter Stop: {st.session_state.selected_stop_id}")