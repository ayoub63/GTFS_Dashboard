from __future__ import annotations

import requests
import streamlit as st


def get_base_url() -> str:
    return st.sidebar.text_input("API Base URL", value=st.session_state.get("api_base_url", "http://localhost:5000"), key="api_base_url")


@st.cache_data(ttl=60)
def fetch_json(url: str, params: dict | None = None):
    response = requests.get(url, params=params, timeout=30)
    response.raise_for_status()
    return response.json()