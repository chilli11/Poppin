import Controller from '@ember/controller';
import { action } from '@ember/object';

export default class VendorsAddController extends Controller {
	@action
	redirectToVendor(vendor) {
		this.transitionToRoute('locations.location', vendor);
	}
}
