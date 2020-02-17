import format from 'date-fns/format';

export const formatFullDateTimeString = (time: Date | string) => format(new Date(time), 'EEEE, MMMM d, yyyy');