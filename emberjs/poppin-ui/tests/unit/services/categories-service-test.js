import { module, test } from 'qunit';
import { setupTest } from 'ember-qunit';

module('Unit | Service | categories-service', function(hooks) {
  setupTest(hooks);

  // Replace this with your real tests.
  test('it exists', function(assert) {
    let service = this.owner.lookup('service:categories-service');
    assert.ok(service);
  });
});
