from datetime import date

import pandas as pd
import plotly.express as px
import streamlit as st

from api_client import fetch_json, get_base_url

st.title("Analytics")
base_url = get_base_url()

col1, col2 = st.columns(2)
selected_date = col1.date_input("Datum", value=date.today())
weekday_filter = col2.selectbox("Oder Wochentag", ["", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"])
limit = st.number_input("Top Stops Limit", min_value=1, max_value=100, value=10)

params = {"date": selected_date.isoformat(), "limit": limit}
if weekday_filter:
    params["weekday"] = weekday_filter

if st.button("KPI laden"):
    top_stops = fetch_json(f"{base_url}/api/stats/top-stops", params)
    peak_hours = fetch_json(f"{base_url}/api/stats/peak-hours", params)

    if top_stops:
        top_df = pd.DataFrame(top_stops)
        fig = px.bar(top_df, x="stopName", y="count", title="Top Stops")
        st.plotly_chart(fig, use_container_width=True)
        stop_for_routes = st.selectbox("Routes by Stop", top_df["stopId"])
        route_rows = fetch_json(f"{base_url}/api/stats/routes-by-stop/{stop_for_routes}", params)
        st.dataframe(pd.DataFrame(route_rows), use_container_width=True)

    if peak_hours:
        peak_df = pd.DataFrame(peak_hours)
        fig2 = px.bar(peak_df, x="hour", y="count", title="Peak Hours")
        st.plotly_chart(fig2, use_container_width=True)