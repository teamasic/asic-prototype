import format from 'date-fns/format';
import formatDistanceToNow from 'date-fns/formatDistanceToNow';
import moment from 'moment';
import Swal from 'sweetalert2';
import uniqid from 'uniqid';

export const formatFullDateTimeString = (time: Date | string) => format(new Date(time), 'EEEE, MMMM d, yyyy');

export const formatDateDDMMYYYYHHmm = (time: Date | string) => format(new Date(time), 'dd-MM-yyyy hh:mm');

export const minutesOfDay = (time: moment.Moment) => {
    return time.minutes() + time.hours() * 60;
}
export const formatDateString = (time: Date | string) => format(new Date(time), 'dd/MM/yyyy');

export const renderStripedTable = (record: any, index: number) => {
    if (index % 2 == 0) {
        return 'default';
    } else {
        return 'striped';
    }
}

export const success = (msg: string) => {
    Swal.fire({
        icon: 'success',
        text: msg
    });
}

export const error = (msg: string) => {
    Swal.fire({
        icon: 'error',
        text: msg
    });
}

export const formatDateDistanceToNow = (time: Date | string) => formatDistanceToNow(new Date(time));

export const formatTimeOnly = (time: Date | string) => format(new Date(time), 'hh:mm');

export const generateUniqueId = () => uniqid.time();