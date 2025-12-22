import httpClient from "./httpClient";

async function getReportStatesById(idNaloga){
    const {data} = await httpClient.get(`api/dailyreports/${idNaloga}/state`);
    return data;
}

async function getOrCreateReportByDate(datum){
    const {data} = await httpClient.post("api/dailyreports/for-date", {datum});
    return data;
}

async function putMeasuredProsuto(idNaloga, prosutoKanta){
    const {data} = (await httpClient.put(`api/dailyreports/${idNaloga}/prosuto-kanta`, {prosutoKanta})).data;
    return data;
}

async function calculateProsutoRazlika(idNaloga){
    const {data} = await httpClient.post(`api/dailyreports/${idNaloga}/calculate-prosuto-razlika`);
    return data;
}


export {
    getReportStatesById,
    getOrCreateReportByDate,
    putMeasuredProsuto,
    calculateProsutoRazlika
};