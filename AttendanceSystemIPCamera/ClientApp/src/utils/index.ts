import format from 'date-fns/format';
import moment from 'moment';

export const formatFullDateTimeString = (time: Date | string) => format(new Date(time), 'EEE, dd/MM/yyyy');
export const minutesOfDay = (time: moment.Moment) => {
    return time.minutes() + time.hours() * 60;
}
export const formatDateString = (time: Date | string) => format(new Date(time), 'dd/MM/yyyy');