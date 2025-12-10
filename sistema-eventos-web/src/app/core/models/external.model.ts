// --- Pagos ---
export interface PaymentIntentRequest {
  reservationId: string;
}

export interface PaymentResponse {
  paymentUrl: string;
  transactionId: string;
}

// --- Clima ---
export interface WeatherResponse {
  main: {
    temp: number;
    humidity: number;
  };
  weather: Array<{
    description: string;
    main: string;
  }>;
  name: string; // Ciudad
}

// --- Geolocalización (Nominatim) ---
export interface GeoResponse {
  lat: string;
  lon: string;
  display_name: string;
}