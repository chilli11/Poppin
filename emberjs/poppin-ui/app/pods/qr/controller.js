import Controller from '@ember/controller';
import { tracked } from '@glimmer/tracking';

export default class QrController extends Controller {
	queryParams = ['loc_id'];

	@tracked loc_id;
}
