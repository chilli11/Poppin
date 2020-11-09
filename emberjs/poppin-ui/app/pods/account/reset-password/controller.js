import Controller from '@ember/controller';
import { tracked } from '@glimmer/tracking';
import { action } from '@ember/object';

export default class AccountResetPasswordController extends Controller {
	queryParams = ['t'];

	@tracked t;
}
