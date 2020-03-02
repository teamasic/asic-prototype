import format from 'date-fns/format';

export const formatFullDateTimeString = (time: Date | string) => format(new Date(time), 'EEE, dd/MM/yyyy');

export const formatDateString = (time: Date | string) => format(new Date(time), 'dd/MM/yyyy');