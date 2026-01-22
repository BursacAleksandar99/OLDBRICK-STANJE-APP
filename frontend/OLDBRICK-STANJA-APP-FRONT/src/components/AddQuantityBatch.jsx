import React, { useState, useEffect, useMemo } from "react";
import { addMoreBeerQuantity } from "../api/helpers";

function AddQuantityBatch({ idNaloga, articles = [], onUpdated }) {
  console.log(articles);

  const [beerItems, setBeerItems] = useState(
    articles.map((a) => {
      if (!a.id) {
        console.error("Ovo pivo nema idPiva!", a);
      }
      return { idPiva: a.id, kolicina: "", nazivPiva: a.nazivPiva };
    }),
  );

  console.log(beerItems);

  const [loading, setLoading] = useState(false);
  const [msg, setMsg] = useState("");

  function handleKolicinaChange(idPiva, value) {
    setBeerItems((prev) => {
      const newState = prev.map((item) => {
        if (item.idPiva === idPiva) {
          return { ...item, kolicina: Number(value) };
        } else {
          return item;
        }
      });
      console.log("beerItems after change:", newState);

      return newState;
    });
  }

  async function handleBatchAdd() {
    try {
      setMsg("");

      // filterujemo samo piva gde je input popunjen
      const payload = beerItems
        .filter((item) => {
          return item.kolicina !== "" && Number(item.kolicina) > 0;
        })
        .map((item) => {
          return { idPiva: item.idPiva, kolicina: Number(item.kolicina) };
        });

      if (payload.length === 0) {
        return setMsg("Nema unetih količina.");
      }
      const idNalogaDayBefore = idNaloga;

      setLoading(true);
      const updated = await addMoreBeerQuantity(idNalogaDayBefore, payload);

      // reset input polja
      setBeerItems(function (prev) {
        return prev.map((item) => {
          return { ...item, kolicina: "" };
        });
      });

      setMsg("Dodato ✅");
      onUpdated?.(updated);
    } catch (e) {
      console.error(e);
      setMsg("Greška pri dodavanju.");
    } finally {
      setLoading(false);
      setTimeout(() => {
        setMsg("");
      }, 2500);
    }
  }

  const articleOrder = [
    "Stara cigla svetla",
    "Stara cigla IPA",
    "Nektar",
    "Haineken",
    "Paulaner svetli",
    "Paulaner psenica",
    "Kozel tamno",
    "Blank",
    "Tuborg",
    "Kafa",
  ];
  const displayBeerItems = React.useMemo(() => {
    return [...beerItems].sort((a, b) => {
      const ia = articleOrder.indexOf(a.nazivPiva);
      const ib = articleOrder.indexOf(b.nazivPiva);

      if (ia === -1 && ib === -1) return 0;
      if (ia === -1) return 1;
      if (ib === -1) return -1;

      return ia - ib;
    });
  }, [beerItems]);

  return (
    <div className="flex flex-col gap-2">
      {displayBeerItems.map(function (item) {
        return (
          <div
            key={item.idPiva}
            className="flex items-center justify-between gap-4"
          >
            <span className="flex-1">{item.nazivPiva}</span>
            <input
              type="text"
              inputMode="decimal"
              value={item.kolicina}
              onChange={(e) => {
                const val = e.target.value;

                if (/^\d*\.?\d*$/.test(val)) {
                  handleKolicinaChange(item.idPiva, val);
                }
              }}
              className="w-20 border px-2 py-1 rounded text-right"
            />
          </div>
        );
      })}

      <button
        type="button"
        onClick={handleBatchAdd}
        disabled={loading}
        className="mt-4 rounded-lg bg-[#FACC15] px-4 py-2 font-semibold text-black
                 transition hover:bg-[#fde047] disabled:opacity-50 disabled:cursor-not-allowed"
      >
        {loading ? "Dodajem..." : "Dodaj sve"}
      </button>

      <p className="mt-2 text-sm text-green-600">{msg}</p>
    </div>
  );
}

export default AddQuantityBatch;
