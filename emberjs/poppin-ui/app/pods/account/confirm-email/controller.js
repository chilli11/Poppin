import Controller from '@ember/controller';
import { tracked } from '@glimmer/tracking';

export default class AccountConfirmEmailController extends Controller {
	queryParams = ['t'];

	@tracked t;
}
